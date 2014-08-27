using System;
using System.Collections.Generic;
using System.Management;

namespace WMI_Explorer
{
    class WMI
    {
        public static List<string> getNameSpaces()
        {
            ManagementClass nsClass = new ManagementClass(new ManagementScope("root"), new ManagementPath("__namespace"), null);
            var nameSpaces = new List<string>();

            foreach (ManagementObject ns in nsClass.GetInstances())
            {
                nameSpaces.Add(ns["Name"].ToString());
            }

            return nameSpaces;
        }

        public static List<string> getClasses(string className)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope("root\\" + className),
                new WqlObjectQuery("select * from meta_class"), null);
            var classes = new List<string>();

            foreach (ManagementClass wmiClass in searcher.Get())
            {
                classes.Add(wmiClass["__CLASS"].ToString());
            }

            return classes;
        }

        public static List<string> searchClasses(string className, string search)
        {
            var classes = new List<string>();

            foreach (string wmiClass in getClasses(className))
            {
                if (wmiClass.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    classes.Add(wmiClass);
            }

            return classes;
        }
    }
}
