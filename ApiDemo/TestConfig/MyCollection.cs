﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiDemo.TestConfig
{
    [CollectionDefinition("MyCollection")]
    public class MyCollection: ICollectionFixture<MyHttpClient>
    {
    }
}
