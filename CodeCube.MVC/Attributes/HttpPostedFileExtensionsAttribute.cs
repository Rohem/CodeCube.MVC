﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CodeCube.Mvc.Attributes
{
    public class HttpPostedFileExtensionsAttribute : DataTypeAttribute, IClientValidatable
    {
        private readonly FileExtensionsAttribute _innerAttribute = new FileExtensionsAttribute();

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpPostedFileExtensionsAttribute" /> class.
        /// </summary>
        public HttpPostedFileExtensionsAttribute()
            : base(DataType.Upload)
        {
            ErrorMessage = _innerAttribute.ErrorMessage;
        }

        /// <summary>
        ///     Gets or sets the file name extensions.
        /// </summary>
        /// <returns>
        ///     The file name extensions, or the default file extensions (".png", ".jpg", ".jpeg", and ".gif") if the property is not set.
        /// </returns>
        public string Extensions
        {
            get => _innerAttribute.Extensions;
            set => _innerAttribute.Extensions = value;
        }

        /// <summary>
        /// The name of the property the attribute is placed on.
        /// </summary>
        public string PropertyName { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "accept",
                ErrorMessage = ErrorMessage
            };

            rule.ValidationParameters["exts"] = _innerAttribute.Extensions;
            yield return rule;
        }

        /// <summary>
        ///     Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <returns>
        ///     The formatted error message.
        /// </returns>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        public override string FormatErrorMessage(string name)
        {
            return _innerAttribute.FormatErrorMessage(name);
        }

        /// <summary>
        ///     Checks that the specified file name extension or extensions is valid.
        /// </summary>
        /// <returns>
        ///     true if the file name extension is valid; otherwise, false.
        /// </returns>
        /// <param name="value">A comma delimited list of valid file extensions.</param>
        public override bool IsValid(object value)
        {
            //If value is null there is no file to be validated, so we return true;
            if (value == null) return true;

            if (value is HttpPostedFile file)
            {
                return _innerAttribute.IsValid(file.FileName);
            }

            //Value can be null for some reason, so look for a context.
            if (HttpContext.Current != null && HttpContext.Current.Request.Files.Count > 0)
            {
                file = HttpContext.Current.Request.Files.Get(PropertyName);
                if (file != null)
                {
                    return _innerAttribute.IsValid(file.FileName);
                }
            }

            //If we came here something is wrong.
            return _innerAttribute.IsValid(value);
        }
    }
}