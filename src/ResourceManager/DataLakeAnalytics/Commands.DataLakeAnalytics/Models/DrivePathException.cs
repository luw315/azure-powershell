namespace Microsoft.Azure.Commands.DataLakeAnalytics.Models
{
    [System.Serializable]
    public class DrivePathException : System.Exception
    {
        public DrivePathException() { }
        public DrivePathException(string message) : base(message) { }
        public DrivePathException(string message, System.Exception inner) : base(message, inner) { }
        protected DrivePathException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}