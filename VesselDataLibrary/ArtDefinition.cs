using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Xml;
using RussLibrary;
using System.Windows;
namespace VesselDataLibrary
{
    [XmlConversionRoot("art")]
    public class ArtDefinition : DependencyObject
    {
        //<art     meshfile="dat/artemis.dxs"    diffuseFile="dat/artemis_diffuse.png"    
        //  glowFile="dat/artemis_illum.png"    specularFile="dat/artemis_specular.png" scale="0.2" pushRadius="150"/>


        public static readonly DependencyProperty MeshFileProperty =
            DependencyProperty.Register("MeshFile", typeof(string),
            typeof(ArtDefinition));
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
                              typeof(ArtDefinition));
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
            typeof(ArtDefinition));
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
            typeof(ArtDefinition));
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
            typeof(ArtDefinition));
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


        public static readonly DependencyProperty PushRadiusProperty =
            DependencyProperty.Register("PushRadius", typeof(int),
            typeof(ArtDefinition));
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




    }
}
