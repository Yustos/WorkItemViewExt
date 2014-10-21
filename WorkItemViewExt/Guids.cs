// Guids.cs
// MUST match guids.h
using System;

namespace YL.WorkItemViewExt
{
    static class GuidList
    {
        public const string guidWorkItemViewExtPkgString = "af4a407a-3486-40c5-975d-9dde883ff0b5";
        public const string guidWorkItemViewExtCmdSetString = "106d7db2-7b92-442f-92d4-96d229406300";

        public static readonly Guid guidWorkItemViewExtCmdSet = new Guid(guidWorkItemViewExtCmdSetString);
    };
}