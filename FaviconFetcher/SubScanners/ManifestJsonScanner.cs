﻿using FaviconFetcher.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.SubScanners
{
    class ManifestJsonScanner : SubScanner
    {
        public ManifestJsonScanner(ISource source, Uri uri) : base(source, uri)
        {
        }

        public override void Start()
        {
            using (var reader = Source.DownloadText(TargetUri))
            {
                if (reader != null)
                    _ParseContent(reader);
            }
        }


        [DataContract]
        class IconEntry
        {
            [DataMember]
            public string src { get; set; }

            [DataMember]
            public string sizes { get; set; }
        }

        [DataContract]
        class IconList
        {
            [DataMember]
            public IconEntry[] icons { get; set; }
        }

        private void _ParseContent(StreamReader reader)
        {
            IconList iconList;
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(IconList));
                iconList = serializer.ReadObject(reader.BaseStream) as IconList;
            }
            catch (SerializationException)
            {
                return;
            }

            foreach (var icon in iconList.icons)
            {
                var parts = icon.sizes.Split('x');
                if (parts.Length != 2)
                    continue;

                if (!int.TryParse(parts[0], out int width))
                    continue;
                if (!int.TryParse(parts[1], out int height))
                    continue;

                try
                {
                    Results.Add(new ScanResult
                    {
                        ExpectedSize = new Size(width, height),
                        Location = new Uri(TargetUri, icon.src)
                    });
                }
                catch (UriFormatException) { }
            }
        }
    }
}
