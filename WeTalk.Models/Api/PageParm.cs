﻿/*
* ==============================================================================
*
* FileName: PageParm.cs
* Created: 2020/5/31 21:34:53
* Author: Meiam
* Description: 
*
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace WeTalk.Models
{
    public class PageParm
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页总条数
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 多个排序字段
        /// </summary>
        public string Sort { get; set; }
    }
}
