﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wbooru.Kernel.ProgramUpdater
{
    public class ReleaseInfo
    {
        public enum ReleaseType
        {
            Stable, Preview
        }

        public ReleaseType Type { get; set; }
        public Version Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string DownloadURL { get; set; }
        public string ReleaseURL { get; set; }
        public string Description { get; set; }
    }
}
