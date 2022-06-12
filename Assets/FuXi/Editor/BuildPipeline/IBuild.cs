using System;
// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    internal interface IBuild : IDisposable
    {
        public void BeginBuild();
        public void EndBuild();

        public void OnAssetValueChanged();
    }
}