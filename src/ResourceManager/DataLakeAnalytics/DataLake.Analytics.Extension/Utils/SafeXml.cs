namespace Microsoft.Azure.Management.DataLake.Analytics.Extension.Utils
{
    using System.IO;
    using System.Xml;

    /// <summary>
    /// SafeXml
    ///  OACR flags a number of issues with System.Xml
    ///  This helper class should be used instead for the problematic functions:
    ///     63053: UseXmlSecureResolver http://oacrhelp/?warning=63053
    ///     63056: UseXmlReaderForLoad  http://oacrhelp/?warning=63056
    ///     63057: DoNotUseLoadXml      http://oacrhelp/?warning=63057
    /// </summary>
    internal static class SafeXml
    {
        /// <summary>
        ///  new XmlDocument()
        ///     63053   UseXmlSecureResolver    http://oacrhelp/?warning=63053
        /// </summary>
        /// <remarks>
        ///     Review code to ensure that external resource resolution is explicitly disabled or a XmlSecureResolver is used when processing 
        ///     untrusted input (the resolver used internally on some overloaded methods is not safe to use on untrusted input). 
        ///     Using default resolver for resolving external XML entities may lead to information disclosure. 
        ///     Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlDocument CreateXmlDocument()
        {
            // Do not change XmlResolver from null unless it is being changed to use XmlSecureResolver. Using
            // the default XmlResolver class could allow external resources to be resolved as part of reading
            // the Xml. External XML's can be used to cause DoS or data breach. For more info, see:
            // https://msdn.microsoft.com/en-us/library/5fcwybb2(v=vs.110).aspx
            // Note: Due to inconsistent OACR scans:
            //  http://oacrhelp:82/oacr-answer/1115 we purposely use the object intializer here
            return new XmlDocument { XmlResolver = null };
        }

        /// <summary>
        ///  XmlDocument.LoadXml(...)
        ///     63057   DoNotUseLoadXml         http://oacrhelp/?warning=63057
        /// </summary>
        /// <remarks>
        ///     Do not use unsafe overloads of System.Xml.XmlDocument/XmlDataDocument LoadXml API. 
        ///     This API internally enables DTD processing on the XML reader instance used, and uses UrlResolver for resolving external XML entities. 
        ///     The outcome is information disclosure. Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlDocument LoadFromXml(string xml)
        {
            using (var reader = SafeXml.CreateXmlReaderFromXml(xml))
            {
                return SafeXml.LoadSafeXmlDocument(reader);
            }
        }

        /// <summary>
        ///  XmlDocument.LoadXml(...)
        ///     63057   DoNotUseLoadXml         http://oacrhelp/?warning=63057
        /// </summary>
        /// <remarks>
        ///     Do not use unsafe overloads of System.Xml.XmlDocument/XmlDataDocument LoadXml API. 
        ///     This API internally enables DTD processing on the XML reader instance used, and uses UrlResolver for resolving external XML entities. 
        ///     The outcome is information disclosure. Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlDocument LoadFromXml(byte[] xml)
        {
            using (var reader = SafeXml.CreateXmlReaderFromXml(xml))
            {
                return SafeXml.LoadSafeXmlDocument(reader);
            }
        }

        /// <summary>
        ///  XmlDocument.Load(...)
        ///     63056   UseXmlReaderForLoad     http://oacrhelp/?warning=63056
        /// </summary>
        /// <remarks>
        ///     Do not use unsafe overloads of System.Xml.XmlDocument/XmlDataDocument Load. 
        ///     This API internally enables DTD processing on the XML reader instance used, and uses UrlResolver for resolving external XML entities. 
        ///     The outcome is information disclosure. Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlDocument LoadFromFile(string filename)
        {
            using (var reader = SafeXml.CreateXmlReaderFromFile(filename))
            {
                return SafeXml.LoadSafeXmlDocument(reader);
            }
        }

        /// <summary>
        ///  XmlDocument.Load(...)
        ///     63056   UseXmlReaderForLoad     http://oacrhelp/?warning=63056
        /// </summary>
        /// <remarks>
        ///     Do not use unsafe overloads of System.Xml.XmlDocument/XmlDataDocument Load. 
        ///     This API internally enables DTD processing on the XML reader instance used, and uses UrlResolver for resolving external XML entities. 
        ///     The outcome is information disclosure. Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlDocument LoadFromStream(Stream stream)
        {
            using (var reader = SafeXml.CreateXmlReaderFromStream(stream))
            {
                return SafeXml.LoadSafeXmlDocument(reader);
            }
        }

        /// <summary>
        ///  XmlReader.Create(...)
        ///     63053   UseXmlSecureResolver    http://oacrhelp/?warning=63053
        /// </summary>
        /// <remarks>
        ///     Review code to ensure that external resource resolution is explicitly disabled or a XmlSecureResolver is used when processing 
        ///     untrusted input (the resolver used internally on some overloaded methods is not safe to use on untrusted input). 
        ///     Using default resolver for resolving external XML entities may lead to information disclosure. 
        ///     Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlReader CreateXmlReaderFromXml(string xml, XmlReaderSettings settings = null)
        {
            return XmlReader.Create(new StringReader(xml), SafeXml.CreateXmlReaderSettings(settings, closeinput: true));
        }

        /// <summary>
        ///  XmlReader.Create(...)
        ///     63053   UseXmlSecureResolver    http://oacrhelp/?warning=63053
        /// </summary>
        /// <remarks>
        ///     Review code to ensure that external resource resolution is explicitly disabled or a XmlSecureResolver is used when processing 
        ///     untrusted input (the resolver used internally on some overloaded methods is not safe to use on untrusted input). 
        ///     Using default resolver for resolving external XML entities may lead to information disclosure. 
        ///     Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlReader CreateXmlReaderFromXml(byte[] xml, XmlReaderSettings settings = null)
        {
            return XmlReader.Create(new MemoryStream(xml), SafeXml.CreateXmlReaderSettings(settings, closeinput: true));
        }

        /// <summary>
        ///  XmlReader.Create(...)
        ///     63053   UseXmlSecureResolver    http://oacrhelp/?warning=63053
        /// </summary>
        /// <remarks>
        ///     Review code to ensure that external resource resolution is explicitly disabled or a XmlSecureResolver is used when processing 
        ///     untrusted input (the resolver used internally on some overloaded methods is not safe to use on untrusted input). 
        ///     Using default resolver for resolving external XML entities may lead to information disclosure. 
        ///     Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlReader CreateXmlReaderFromFile(string filename, XmlReaderSettings settings = null)
        {
            return XmlReader.Create(filename, SafeXml.CreateXmlReaderSettings(settings));
        }

        /// <summary>
        ///  XmlReader.Create(...)
        ///     63053   UseXmlSecureResolver    http://oacrhelp/?warning=63053
        /// </summary>
        /// <remarks>
        ///     Review code to ensure that external resource resolution is explicitly disabled or a XmlSecureResolver is used when processing 
        ///     untrusted input (the resolver used internally on some overloaded methods is not safe to use on untrusted input). 
        ///     Using default resolver for resolving external XML entities may lead to information disclosure. 
        ///     Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlReader CreateXmlReaderFromStream(Stream input, XmlReaderSettings settings = null)
        {
            return XmlReader.Create(input, SafeXml.CreateXmlReaderSettings(settings, closeinput: true));
        }

        /// <summary>
        ///  XmlReader.Create(...)
        ///     63053   UseXmlSecureResolver    http://oacrhelp/?warning=63053
        /// </summary>
        /// <remarks>
        ///     Review code to ensure that external resource resolution is explicitly disabled or a XmlSecureResolver is used when processing 
        ///     untrusted input (the resolver used internally on some overloaded methods is not safe to use on untrusted input). 
        ///     Using default resolver for resolving external XML entities may lead to information disclosure. 
        ///     Content from file system or network shares for the machine processing the XML can be exposed to attacker. 
        ///     In addition, an attacker can use this as a DoS vector.
        /// </remarks>
        internal static XmlReader CreateXmlReaderFromReader(TextReader reader, XmlReaderSettings settings = null)
        {
            return XmlReader.Create(reader, SafeXml.CreateXmlReaderSettings(settings, closeinput: true));
        }

        /// <summary/>
        internal static XmlReaderSettings CreateXmlReaderSettings(XmlReaderSettings settings = null, bool closeinput = true)
        {
            // Note: Due to inconsistent OACR scans:
            //  http://oacrhelp:82/oacr-answer/1115 we purposely use the object intializer here
            //  Note: CheckCharacters = false, is the default for XmlTextReader in order to support hex escapes: &#x1;
            return settings ?? new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Ignore, CheckCharacters = false, CloseInput = closeinput };
        }

        /// <summary/>
        private static XmlDocument LoadSafeXmlDocument(XmlReader reader)
        {
            var d = SafeXml.CreateXmlDocument();
            d.Load(reader);
            return d;
        }
    }
}
