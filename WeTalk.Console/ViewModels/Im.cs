using System;
using System.Collections.Generic;

#nullable disable

namespace WeTalk.Console.ViewModels
{

    public partial class GetImMessageQ0
    {
        public long Imid { get; set; }
    }
    public partial class GetImMessageQ1
    {
        public long Imid { get; set; }
        public long AccCardid { get; set; }
        public long SendCardid { get; set; }
        public DateTime SendTime { get; set; }
        public string Message { get; set; }
        public int Sty { get; set; }
    }
    public partial class GetImMessageQ2
    {
        public long CustomerCardid { get; set; }
        public DateTime Lasttime { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }
    }
    public partial class GetImMessageQ3
    {
        public long Cardid { get; set; }
        public long Userid { get; set; }
        public string HeadImg { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int IsopenEmail { get; set; }
        public int IsopenMobile { get; set; }
        public int IsopenWeixin { get; set; }
        public string Weixin { get; set; }
        public string MapAddress { get; set; }
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public long Companyid { get; set; }
        public string Company { get; set; }
    }
}
