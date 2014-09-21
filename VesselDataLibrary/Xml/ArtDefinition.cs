using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Xml;
using RussLibrary;
using System.Windows;
using RussLibrary.WPF;
using ArtemisModLoader;
using RussLibrary.Helpers;
using System.Globalization;
using System.Xml;
namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("art")]
    public class ArtDefinition : ChangeDependencyObject, IXmlStorage
    {
        //<art     meshfile="dat/artemis.dxs"    diffuseFile="dat/artemis_diffuse.png"    
        //  glowFile="dat/artemis_illum.png"    specularFile="dat/artemis_specular.png" scale="0.2" pushRadius="150"/>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArtDefinition()
        {
            Storage = new List<XmlNode>();
        }
        public static readonly DependencyProperty MeshFileProperty =
            DependencyProperty.Register("MeshFile", typeof(string),
            typeof(ArtDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("meshfile")]
        public string MeshFile
        {
            get
            {
                return (string)this.UIThreadGetValue(MeshFileProperty);

            }
            set
            {
                this.UIThreadSetValue(MeshFileProperty, value);

            }
        }

        public static readonly DependencyProperty DiffuseFileProperty =
            DependencyProperty.Register("DiffuseFile", typeof(string),
            typeof(ArtDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("diffuseFile")]
        public string DiffuseFile
        {
            get
            {
                return (string)this.UIThreadGetValue(DiffuseFileProperty);
            }
            set
            {
                this.UIThreadSetValue(DiffuseFileProperty, value);
            }
        }


        public static readonly DependencyProperty GlowFileProperty =
            DependencyProperty.Register("GlowFile", typeof(string),
            typeof(ArtDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("glowFile")]
        public string GlowFile
        {
            get
            {
                return (string)this.UIThreadGetValue(GlowFileProperty);
            }
            set
            {
                this.UIThreadSetValue(GlowFileProperty, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Specular")]
        public static readonly DependencyProperty SpecularFileProperty =
            DependencyProperty.Register("SpecularFile", typeof(string),
            typeof(ArtDefinition), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Specular"), XmlConversion("specularFile")]
        public string SpecularFile
        {
            get
            {
                return (string)this.UIThreadGetValue(SpecularFileProperty);
            }
            set
            {
                this.UIThreadSetValue(SpecularFileProperty, value);
            }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double),
            typeof(ArtDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("scale")]
        public double Scale
        {
            get
            {
                return (double)this.UIThreadGetValue(ScaleProperty);
            }
            set
            {
                this.UIThreadSetValue(ScaleProperty, value);
            }
        }
        static void OnPushRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtDefinition me = sender as ArtDefinition;
            if (me != null)
            {
                if (!me.ShipSizeUpdating)
                {
                    me.ShipSizeUpdating = true;
                    me.ShipSize = me.PushRadius * 2;
                    me.ShipSizeUpdating = false;
                  
                }
                ArtDefinition.OnItemChanged(me, e);
            }

        }

        public static readonly DependencyProperty PushRadiusProperty =
            DependencyProperty.Register("PushRadius", typeof(int),
            typeof(ArtDefinition), new PropertyMetadata(OnPushRadiusChanged));
        [XmlConversion("pushRadius")]
        public int PushRadius
        {
            get
            {
                return (int)this.UIThreadGetValue(PushRadiusProperty);
            }
            set
            {
                this.UIThreadSetValue(PushRadiusProperty, value);
            }
        }
        static void OnShipSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtDefinition me = sender as ArtDefinition;
            if (me != null)
            {
                if (!me.ShipSizeUpdating)
                {
                    me.ShipSizeUpdating = true;
                    me.PushRadius = me.ShipSize / 2;
                    me.ShipSizeUpdating = false;
                }
            }

        }
        bool ShipSizeUpdating = false;
        public static readonly DependencyProperty ShipSizeProperty =
            DependencyProperty.Register("ShipSize", typeof(int),
            typeof(ArtDefinition), new PropertyMetadata(OnShipSizeChanged));
        
        public int ShipSize
        {
            get
            {
                return (int)this.UIThreadGetValue(ShipSizeProperty);
            }
            set
            {
                this.UIThreadSetValue(ShipSizeProperty, value);
            }
        }




        protected override void ProcessValidation()
        {
            if (string.IsNullOrEmpty(this.MeshFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.MeshFile, ValidationValue.IsError,
                        string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsRequired, AMLResources.Properties.Resources.MeshFile));
            }
            else if (!MeshFile.EndsWith(DataStrings.DXSExtension, StringComparison.OrdinalIgnoreCase))
            {
                base.ValidationCollection.AddValidation(DataStrings.MeshFile, ValidationValue.IsWarnState,
                        AMLResources.Properties.Resources.MeshFileExtensionValidation);
            }
            if (string.IsNullOrEmpty(this.DiffuseFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.DiffuseFile, ValidationValue.IsError,
                        string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsRequired, AMLResources.Properties.Resources.DiffuseFile));
            }
            else if (!FileHelper.IsImageFile(DiffuseFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.DiffuseFile, ValidationValue.IsError,
                   AMLResources.Properties.Resources.NotAnImageFile);
            }
            


            if (string.IsNullOrEmpty(this.GlowFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.GlowFile, ValidationValue.IsError,
                        string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsRequired, AMLResources.Properties.Resources.GlowFile));
            }
            else if (!FileHelper.IsImageFile(GlowFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.GlowFile, ValidationValue.IsError,
                   AMLResources.Properties.Resources.NotAnImageFile);
            }


            if (string.IsNullOrEmpty(this.SpecularFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.SpecularFile, ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsRequired, AMLResources.Properties.Resources.SpecularFile));
            }
            else if (!FileHelper.IsImageFile(SpecularFile))
            {
                base.ValidationCollection.AddValidation(DataStrings.SpecularFile, ValidationValue.IsError,
                   AMLResources.Properties.Resources.NotAnImageFile);
            }

            if (Scale <= 0)
            {
                base.ValidationCollection.AddValidation(DataStrings.Scale, ValidationValue.IsError,
                    AMLResources.Properties.Resources.ScaleValidation);
            }
            if (PushRadius <= 0)
            {
                base.ValidationCollection.AddValidation(DataStrings.PushRadius, ValidationValue.IsWarnState,
                    AMLResources.Properties.Resources.PushRadiusValidation);
            }
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
