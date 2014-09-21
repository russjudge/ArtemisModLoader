using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;
using System.Collections.ObjectModel;

namespace ArtemisModLoader.Xml
{
    [XmlConversionRoot("SubMod")]
    public class SubMod : ChangeDependencyObject
    {
        public SubMod()
        {
            Files = new FileGroup();
        }
        public override void AcceptChanges()
        {
            Files.AcceptChanges();
            base.AcceptChanges();
        }
        public override void BeginInitialization()
        {
            base.BeginInitialization();
            Files.BeginInitialization();
        }
        public override void EndInitialization()
        {
            Files.EndInitialization();
            base.EndInitialization();
        }
        public override void RejectChanges()
        {
            base.RejectChanges();
            Files.EndInitialization();
        }
        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(FileGroup),
            typeof(SubMod));
        [XmlConversion("Files")]
        public FileGroup Files
        {
            get
            {
                return (FileGroup)this.UIThreadGetValue(FilesProperty);

            }
            private set
            {
                this.UIThreadSetValue(FilesProperty, value);

            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(SubMod), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("Title")]
        public string Title
        {
            get
            {
                return (string)this.UIThreadGetValue(TitleProperty);

            }
            private set
            {
                this.UIThreadSetValue(TitleProperty, value);

            }
        }


        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool),
            typeof(SubMod), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("IsActive")]
        public bool IsActive
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsActiveProperty);

            }
            set
            {
                this.UIThreadSetValue(IsActiveProperty, value);

            }
        }

        protected override void ProcessValidation()
        {
            if (string.IsNullOrEmpty(this.Title))
            {
                base.ValidationCollection.AddValidation(DataStrings.Title, ValidationValue.IsError,
                        AMLResources.Properties.Resources.TitleVariationValidation);
            }
           
        }
    }
}
