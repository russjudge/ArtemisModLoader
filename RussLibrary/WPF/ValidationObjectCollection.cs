using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Collections.ObjectModel;

namespace RussLibrary.WPF
{

    public class ValidationObjectCollection : ObservableCollection<ValidationObject>
    {
        /// <summary>
        /// Adds the validation.  Tests to make sure that a validation response is only added once.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public void AddValidation(string propertyName, ValidationValue code, string message)
        {
            bool canAdd = true;
            foreach (ValidationObject v in this)
            {
                if (v.PropertyName == propertyName && v.Code == code && v.Message == message)
                {
                    canAdd = false;
                    break;
                }
            }
            if (canAdd)
            {
                this.Add(new ValidationObject(propertyName, ValidationValue.IsError, message));
            }
        }
        public void ClearValidation(string propertyName)
        {
            List<ValidationObject> objectsToRemove = new List<ValidationObject>();
            foreach (ValidationObject v in this)
            {
                if (v.PropertyName == propertyName)
                {
                    objectsToRemove.Add(v);
                }
            }
            foreach (ValidationObject v in objectsToRemove)
            {
                this.Remove(v);
            }
        }
        /// <summary>
        /// Gets the validation result, all validations combined.
        /// </summary>
        /// <returns></returns>
        public ValidationObject GetValidationResult()
        {
            return GetValidationResult(null);
        }
        public ValidationObject GetValidationResult(string propertyName)
        {
            ValidationObject retVal = new ValidationObject(propertyName, ValidationValue.IsValid, string.Empty);
            
            foreach (ValidationObject v in this)
            {
                retVal.MergeValidation(v);
               
            }
            return retVal;
        }
        public ValidationValue ValidationValue
        {
            get
            {

                return GetValidationResult().Code;
            }
        }
    }
}
