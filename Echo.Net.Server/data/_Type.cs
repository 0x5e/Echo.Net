using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class _Type
{
    static Type[] types = { typeof(LoginInfo), typeof(Telnet) };
    public static int type2int(Type type)
    {
        for (int i = 0; i < types.Length; i++)
            if (types[i] == type)
                return i;
        return -1;
    }

    public static Type int2type(int i)
    {
        return types[i];
    }
}
