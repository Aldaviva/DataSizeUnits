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
        /// <exception cref="FileNotFoundException" accessor="get"><paramref name="fileInfo"/> is missing or a directory</exception>
        /// <exception cref="IOException" accessor="get">Failed to read size of file</exception>
        public DataSize FileSize => new(fileInfo.Length);

    }

}