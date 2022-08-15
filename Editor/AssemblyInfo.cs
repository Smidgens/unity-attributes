// smidgens @ github

#if UNITY_EDITOR

using System.Runtime.CompilerServices;

using CFG = Smidgenomics.Unity.Attributes.Config;
[assembly: InternalsVisibleTo(CFG.ASSEMBLY + ".Editor.Tests")]

#endif