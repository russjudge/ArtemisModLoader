using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;

namespace RussLibrary.WPF
{

    /// <summary>
    /// This object is used soley for user information, but can be used to prevent actions,
    /// based on the the Code property.
    /// </summary>
    public class ValidationObject : DependencyObject
    {
        public ValidationObject()
        { }
        public ValidationObject(string propertyName, ValidationValue code, string message)
        {
            LoadValidation(propertyName, code, message);
        }
        
        public void LoadValidation(string propertyName, ValidationValue code, string message)
        {
            PropertyName = propertyName;
            Code = code;
            Message = message;

        }
        public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(ValidationValue),
         typeof(ValidationObject), new UIPropertyMetadata(ValidationValue.IsValid));

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public ValidationValue Code
        {
            get
            {
                return (ValidationValue)this.UIThreadGetValue(ValueProperty);
            }
            set
            {
                this.UIThreadSetValue(ValueProperty, value);
            }
        }
        public static readonly DependencyProperty MessageProperty =
         DependencyProperty.Register("Message", typeof(string),
         typeof(ValidationObject));

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get
            {
                return (string)this.UIThreadGetValue(MessageProperty);
            }
            set
            {
                this.UIThreadSetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty PropertyNameProperty =
         DependencyProperty.Register("PropertyName", typeof(string),
         typeof(ValidationObject));

        /// <summary>
        /// Gets the name of the property. Used to map validation indicators to the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName
        {
            get
            {
                return (string)this.UIThreadGetValue(PropertyNameProperty);
            }
            set
            {
                this.UIThreadSetValue(PropertyNameProperty, value);
            }
        }
        public void MergeValidation(ValidationObject validationObject)
        {
            if (validationObject != null)
            {
                if (this.PropertyName == validationObject.PropertyName || string.IsNullOrEmpty(this.PropertyName))
                {
                    if (this.Code < validationObject.Code)
                    {
                        this.Code = validationObject.Code;
                    }
                    if (!this.Message.Contains(validationObject.Message))
                    {
                        this.Message += "\r\n" + validationObject.Message;
                        if (this.Message.StartsWith("\r\n", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Message = this.Message.Substring(2);
                        }
                    }
                }
            }
        }
    }
}
