using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lotsofone.NBT
{
    public enum TagType
    {
        None = -1,
        
        End = 0,
        
        Byte = 1,
        
        Short = 2,
        
        Int = 3,
        
        Long = 4,
        
        Float = 5,
        
        Double = 6,
        
        ByteArray = 7,
        
        String = 8,
        
        List = 9,
        
        Compound = 10,
        
        IntArray = 11,
        
        LongArray = 12
    }
}
