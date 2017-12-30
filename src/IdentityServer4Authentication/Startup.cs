using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4Authentication.Data;
using IdentityServer4Authentication.Models;
using IdentityServer4Authentication.Services;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using IdentityServer4.Stores;
using IdentityServer4Authentication.Stores;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.AccessTokenValidation;

namespace IdentityServer4Authentication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var db = Configuration.GetConnectionString("db");
            if (db.Equals("sqlite"))
            {
                // Add framework services.
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            }
            if (db.Equals("sqlserver"))
            {
                // Add framework services.
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
            if (db.Equals("psql"))
            {
                // Add framework services.
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add IdentityServer services
            services.AddSingleton<IClientStore, CustomClientStore>();

            services.AddIdentityServer()
                //.AddTemporarySigningCredential() // Can be used for testing until a real cert is available
                // .AddSigningCredential(new X509Certificate2(Path.Combine(".", "certs", "IdentityServer4Auth.pfx")))
                .AddInMemoryApiResources(MyApiResourceProvider.GetAllResources())
                .AddAspNetIdentity<ApplicationUser>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Geo Search API"
                });
                c.DocInclusionPredicate((name, apiDescription) =>
                {
                    var controllerActionDescription = apiDescription.ActionDescriptor as ControllerActionDescriptor;

                    // コントローラ名に Api が含まれていたらSwaggerの対象にする
                    return controllerActionDescription?.ControllerName.Contains("Api") ?? false;
                });
                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    //Flow = "implicit",
                    Flow = "accessCode",
                    AuthorizationUrl = "http://localhost:5000/connect/authorize",
                    //"http://petstore.swagger.io/oauth/dialog",
                    Scopes = new Dictionary<string, string>
                    {
                        //{ "readAccess", "Access read operations" },
                        //{ "writeAccess", "Access write operations" }
                        { "api1", "Access write operations" }
                    }
                });
            });

            services.ConfigureSwaggerGen(options =>
            {
                //options.IncludeXmlComments(pathToDoc);
                options.DescribeAllEnumsAsStrings();
            });

            // データベース作成
            using (var opt = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db2 = opt.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //db2.Database.Migrate();
                db2.Database.EnsureCreated();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, RoleManager<IdentityRole> roleManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Seed database
            InitializeRoles(roleManager).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            //app.UseIdentity();

            // Note that UseIdentityServer must come after UseIdentity in the pipeline
            app.UseIdentityServer();
            /*
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000/",
                AllowedScopes = new List<string> { "api1" },
                //ApiSecret = "secret",
                //ApiName = "dataEventRecords",
                AutomaticAuthenticate = true,
                //SupportedTokens = SupportedTokens.Both,
                // TokenRetriever = _tokenRetriever,
                // required if you want to return a 403 and not a 401 for forbidden responses
                //AutomaticChallenge = true,
                RequireHttpsMetadata = false,    // Set only for development scenarios - not in production
            };

            app.UseIdentityServerAuthentication(identityServerValidationOptions);
            */
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //c.ConfigureOAuth2("ro.client", "secret", "", "");
            });
        }

        // Initialize some test roles. In the real world, these would be setup explicitly by a role manager
        private string[] roles = new[] { "User", "Manager", "Administrator" };
        private async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var newRole = new IdentityRole(role);
                    await roleManager.CreateAsync(newRole);
                    // In the real world, there might be claims associated with roles
                    // await roleManager.AddClaimAsync(newRole, new Claim("foo", "bar"))
                }
            }
        }
    }
}
