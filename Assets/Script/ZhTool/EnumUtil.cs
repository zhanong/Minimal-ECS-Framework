using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

namespace ZhTool
{
    public static class EnumUtil
    {
    }   

    public struct FlagEnumVisitor<TEnum, TConvertor> : IEnumerable<TEnum> where TEnum : Enum where TConvertor : IEnumConvertor<TEnum>
    {
        public TEnum End { get; }
        public bool IncludeEnd { get; }

        public FlagEnumVisitor(TEnum end, bool includeEnd)
        {
            End = end;
            this.IncludeEnd = includeEnd;
        }

        public int Count()
        {
            TConvertor convertor = default;

            int count = 0;
            int value = 1;
            int endValue = convertor.ToInt(End);
            while (value < endValue)
            {
                count++;
                value <<= 1;
            }
            return count + (IncludeEnd ? 1 : 0);
        }
        public IEnumerator<TEnum> GetEnumerator()
        {
            TConvertor convertor = default;
            int value = 1;
            int endValue = convertor.ToInt(End);
            while (value < endValue)
            {
                yield return convertor.ToEnum(value);
                value <<= 1;
            }

            if (IncludeEnd)
                yield return End;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface IEnumConvertor<TEnum> where TEnum : Enum
    {
        public int ToInt(TEnum value);
        public TEnum ToEnum(int value);
    }
}
