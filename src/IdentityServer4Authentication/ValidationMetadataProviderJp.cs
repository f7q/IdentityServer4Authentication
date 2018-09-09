using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace IdentityServer4Authentication
{
    public class ValidationMetadataProviderJp : IValidationMetadataProvider
    {
        private ResourceManager resourceManager; private Type resourceType;
        public ValidationMetadataProviderJp(string baseName, Type type)
        {
            resourceType = type;
            resourceManager = new ResourceManager(baseName,
                type.GetTypeInfo().Assembly);
        }

        public void CreateValidationMetadata(
            ValidationMetadataProviderContext context)
        {
            if (context.Key.ModelType.GetTypeInfo().IsValueType &&
                context.ValidationMetadata.ValidatorMetadata.Where(m => m.GetType() == typeof(RequiredAttribute)).Count() == 0)
                context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute());

            foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
            {
                ValidationAttribute tAttr = attribute as ValidationAttribute;
                if (tAttr != null && tAttr.ErrorMessageResourceName == null)
                {
                    //何故かEmailAddressAttributeはErrorMessageがデフォルトでnullにならない。
                    if (tAttr.ErrorMessage == null || (attribute as EmailAddressAttribute != null && !string.IsNullOrEmpty(tAttr.ErrorMessage)))
                    {
                        var name = tAttr.GetType().Name;
                        if (resourceManager.GetString(name) != null)
                        {
                            tAttr.ErrorMessageResourceType = resourceType;
                            tAttr.ErrorMessageResourceName = name;
                            tAttr.ErrorMessage = null;
                        }
                    }
                }
            }
        }
    }
}

