using System;

namespace WebCrawler
{
    internal class References
    {
        public Uri Url { get; set; }

        public string Name { get; set; }

        public string OriginalRow { get; set; }

        private ReferenceTypeEnum _referenceType;

        public ReferenceTypeEnum ReferenceType
        {
            get
            {
                _referenceType = Url.IsAbsoluteUri ? ReferenceTypeEnum.Absolute : ReferenceTypeEnum.Relative;
                return _referenceType;
            }
            set => _referenceType = value;
        }

        public References(Uri url, string name, string row)
        {
            Url = url;
            Name = name;
            OriginalRow = row;
        }

        private Boolean IsExternalUrl(Uri url)
        {
            return url.IsAbsoluteUri;
        }
    }
}