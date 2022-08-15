// smidgens @ github

#if UNITY_EDITOR
using System.Runtime.CompilerServices;
using CFG = Smidgenomics.Unity.Attributes.Config;
[assembly: InternalsVisibleTo(CFG.ASSEMBLY + ".Editor")]
[assembly: InternalsVisibleTo(CFG.ASSEMBLY + ".Tests")]
#endif