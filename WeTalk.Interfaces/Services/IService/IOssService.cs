using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System;

namespace WeTalk.Interfaces.Services
{
    public partial interface IOssService : IBaseService
    {
        bool DelFile(string img);
        string DoPost();
        string GetPolicyToken(string UploadDir = "");
    }
}