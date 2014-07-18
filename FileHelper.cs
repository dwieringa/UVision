// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class FileHelper
    {
        public static string ExpandPath(Project theProject, string path)
        {
            path = path.Replace("<PROJECT>", theProject.Name);
            //TODO: throw error if this exists: path = path.Replace("<TESTSEQ>", theObject.TestSequence().Name);
            //TODO: throw error if this exists: path = path.Replace("<TOOL>", theObject.TestSequence().Name);

            DateTime now = DateTime.Now;
            path = path.Replace("<YYYYMMDD>", now.ToString("yyyyMMdd"));
            path = path.Replace("<MM>", now.ToString("MM"));
            path = path.Replace("<DD>", now.ToString("dd"));
            path = path.Replace("<YY>", now.ToString("yy"));

            return path;
        }
        public static string ExpandPath(IObjectDefinition theObject, string path)
        {
            path = path.Replace("<PROJECT>", theObject.Project().Name);
            path = path.Replace("<TESTSEQ>", theObject.TestSequence().Name);
            path = path.Replace("<TOOL>", theObject.TestSequence().Name);

            DateTime now = DateTime.Now;
            path = path.Replace("<YYYYMMDD>", now.ToString("yyyyMMdd"));
            path = path.Replace("<MM>", now.ToString("MM"));
            path = path.Replace("<DD>", now.ToString("dd"));
            path = path.Replace("<YY>", now.ToString("yy"));

            return path;
        }
    }
}
