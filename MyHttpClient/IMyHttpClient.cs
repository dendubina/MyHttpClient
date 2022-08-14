﻿using System;
using System.Threading.Tasks;
using MyHttpClientProject.Models;

namespace MyHttpClientProject
{
    public interface IMyHttpClient : IDisposable
    {
        Task<HttpResponse> SendAsync(RequestOptions options);
    }
}
