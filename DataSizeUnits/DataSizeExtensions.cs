using System.ComponentModel;

namespace DataSizeUnits;

/// <summary>
/// Extensions for types to let you easily use <see cref="DataSize"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class DataSizeExtensions {

    extension(FileInfo fileInfo) {

        /// <summary>
        /// The file size or length of this file, represented as a <see cref="DataSize"/> instead of a <see cref="long"/>.
        /// </summary>
        public DataSize FileSize => new(fileInfo.Length);

    }

}