﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services.Interfaces
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}