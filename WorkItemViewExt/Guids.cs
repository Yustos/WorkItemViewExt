// Guids.cs
// MUST match guids.h
using System;

namespace YL.WorkItemViewExt
{
    static class GuidList
    {
        public const string guidWorkItemViewExtPkgString = "8f069d8f-506e-461f-a3d6-e2a76e1e1a8f";
        public const string guidWorkItemViewExtCmdSetString = "68454935-794a-4744-a2ee-20a3b2889a80";

        public static readonly Guid guidWorkItemViewExtCmdSet = new Guid(guidWorkItemViewExtCmdSetString);
    };
}