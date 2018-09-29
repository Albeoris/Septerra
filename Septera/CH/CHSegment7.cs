using System;

public struct CHSegment7
{
    /* 0x00 */ public Int32 ActorInfoPtr; // Will initialize in runtime
    /* 0x04 */ public Byte Type; // 1 => Return -1, 2 => Text, 4 => Return 0, 3 => Continue
    /* 0x05 */ public Byte Segment1Index;
    /* 0x06 */ public Byte Flag3;
    /* 0x07 */ public Byte Flag4;
    /* 0x08 */ public Int32 Unknown3;
    
    /* 0x0C */ public Int16 Unknown4;
    /* 0x0E */ public Int16 Unknown5;
    
    /* 0x10 */ public Int32 Unknown6;
    /* 0x14 */ public Int32 Unknown7;
    /* 0x18 */ public Int32 Unknown8;
    /* 0x1C */ public Int32 Segment1Record;
    /* 0x20 */ public Int32 Unknown10;
    /* 0x24 */ public Int32 Unknown11;

    public override string ToString()
    {
        return $"{Segment1Record:X8}";
    }
}