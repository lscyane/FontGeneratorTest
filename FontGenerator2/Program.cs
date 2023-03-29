// [GPT3.5]
// フォントファイルをC#のクラスにパースするクラスを作ってほしいです。

// フォントファイルをC#でパースするには、TrueTypeフォントの形式に従ってファイルを解析する必要があります。以下は、TrueTypeフォントのファイル構造を解析するためのC#クラスの例です。このクラスは、フォントの名前、ファイルのバージョン、グリフの一覧などの情報を取得することができます。

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TrueTypeFont
{
    private const uint SFNT_MAGIC = 0x5F0F3CF5;
    private const uint CMAP_TABLE_TAG = 0x636D6170;
    private const uint HEAD_TABLE_TAG = 0x68656164;
    private const uint HHEA_TABLE_TAG = 0x68686561;
    private const uint MAXP_TABLE_TAG = 0x6D617870;
    private const uint NAME_TABLE_TAG = 0x6E616D65;
    private const uint POST_TABLE_TAG = 0x706F7374;

    private uint version;
    private ushort numTables;
    private ushort searchRange;
    private ushort entrySelector;
    private ushort rangeShift;

    private Dictionary<uint, uint> tableOffsets = new Dictionary<uint, uint>();
    private Dictionary<uint, uint> tableLengths = new Dictionary<uint, uint>();

    private string fontFamilyName;
    private ushort unitsPerEm;
    private short ascender;
    private short descender;
    private short lineGap;

    private ushort numGlyphs;
    private ushort maxPoints;
    private ushort maxContours;
    private ushort maxCompositePoints;
    private ushort maxCompositeContours;
    private ushort maxZones;
    private ushort maxTwilightPoints;
    private ushort maxStorage;
    private ushort maxFunctionDefs;
    private ushort maxInstructionDefs;
    private ushort maxStackElements;
    private ushort maxSizeOfInstructions;
    private ushort maxComponentElements;
    private ushort maxComponentDepth;

    public TrueTypeFont(string filename)
    {
        using (var stream = new FileStream(filename, FileMode.Open))
        {
            Parse(stream);
        }
    }

    public TrueTypeFont(Stream stream)
    {
        Parse(stream);
    }

    private void Parse(Stream stream)
    {
        using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
        {
            var sfntMagic = reader.ReadUInt32();
            if (sfntMagic != SFNT_MAGIC)
            {
                throw new Exception("Invalid TrueType font file format");
            }

            version = reader.ReadUInt32();
            numTables = reader.ReadUInt16();
            searchRange = reader.ReadUInt16();
            entrySelector = reader.ReadUInt16();
            rangeShift = reader.ReadUInt16();

            for (ushort i = 0; i < numTables; i++)
            {
                var tag = reader.ReadUInt32();
                var checksum = reader.ReadUInt32();
                var offset = reader.ReadUInt32();
                var length = reader.ReadUInt32();

                tableOffsets[tag] = offset;
                tableLengths[tag] = length;
            }

            // Parse font tables
            if (tableOffsets.ContainsKey(HEAD_TABLE_TAG))
            {
                ParseHeadTable(reader, tableOffsets[HEAD_TABLE_TAG]);
            }

            if (tableOffsets.ContainsKey(HHEA_TABLE_TAG))
            {
                ParseHheaTable(reader, tableOffsets[HHEA_TABLE_TAG]);
            }

            if (tableOffsets.ContainsKey(MAXP_TABLE_TAG))
            {
                ParseMaxpTable(reader, tableOffsets[MAXP_TABLE_TAG]);
            }

            if (tableOffsets.ContainsKey(NAME_TABLE_TAG))
            {
                ParseNameTable(reader, tableOffsets[NAME_TABLE_TAG]);
            }

            if (tableOffsets.ContainsKey(POST_TABLE_TAG))
            {
                ParsePostTable(reader, tableOffsets[POST_TABLE_TAG]);
            }
        }
    }

    private void ParseHeadTable(BinaryReader reader, uint offset)
    {
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        var version = reader.ReadUInt32();
        var fontRevision = reader.ReadUInt32();
        var checksumAdjustment = reader.ReadUInt32();
        var magicNumber = reader.ReadUInt32();
        var flags = reader.ReadUInt16();
        var unitsPerEm = reader.ReadUInt16();
        var created = reader.ReadInt64();
        var modified = reader.ReadInt64();
        var xMin = reader.ReadInt16();
        var yMin = reader.ReadInt16();
        var xMax = reader.ReadInt16();
        var yMax = reader.ReadInt16();
        var macStyle = reader.ReadUInt16();
        var lowestRecPPEM = reader.ReadUInt16();
        var fontDirectionHint = reader.ReadInt16();
        var indexToLocFormat = reader.ReadInt16();
        var glyphDataFormat = reader.ReadInt16();

        this.unitsPerEm = unitsPerEm;
    }

    private void ParseHheaTable(BinaryReader reader, uint offset)
    {
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        var version = reader.ReadUInt32();
        var ascent = reader.ReadInt16();
        var descent = reader.ReadInt16();
        var lineGap = reader.ReadInt16();
        var advanceWidthMax = reader.ReadUInt16();
        var minLeftSideBearing = reader.ReadInt16();
        var minRightSideBearing = reader.ReadInt16();
        var xMaxExtent = reader.ReadInt16();
        var caretSlopeRise = reader.ReadInt16();
        var caretSlopeRun = reader.ReadInt16();
        var caretOffset = reader.ReadInt16();
        var reserved1 = reader.ReadInt16();
        var reserved2 = reader.ReadInt16();
        var reserved3 = reader.ReadInt16();
        var reserved4 = reader.ReadInt16();
        var metricDataFormat = reader.ReadInt16();
        var numHMetrics = reader.ReadUInt16();

        ascender = ascent;
        descender = descent;
        this.lineGap = lineGap;
    }

    private void ParseMaxpTable(BinaryReader reader, uint offset)
    {
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        var version = reader.ReadUInt32();
        numGlyphs = reader.ReadUInt16();

        if (version >= 0x00010000)
        {
            maxPoints = reader.ReadUInt16();
            maxContours = reader.ReadUInt16();
            maxCompositePoints = reader.ReadUInt16();
            maxCompositeContours = reader.ReadUInt16();
            maxZones = reader.ReadUInt16();
            maxTwilightPoints = reader.ReadUInt16();
            maxStorage = reader.ReadUInt16();
            maxFunctionDefs = reader.ReadUInt16();
            maxInstructionDefs = reader.ReadUInt16();
            maxStackElements = reader.ReadUInt16();
            maxSizeOfInstructions = reader.ReadUInt16();
            maxComponentElements = reader.ReadUInt16();
            maxComponentDepth = reader.ReadUInt16();
        }
    }

    private void ParseNameTable(BinaryReader reader, uint offset)
    {
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        var format = reader.ReadUInt16();
        var count = reader.ReadUInt16();
        var stringOffset = reader.ReadUInt16();

        for (ushort i = 0; i < count; i++)
        {
            var platformID = reader.ReadUInt16();
            var encodingID = reader.ReadUInt16();
            var languageID = reader.ReadUInt16();
            var nameID = reader.ReadUInt16();
            var length = reader.ReadUInt16();
            var offset2 = reader.ReadUInt16();

            var nameOffset = offset + stringOffset + offset2;
            var strBytes = new byte[length];
            reader.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
            reader.Read(strBytes, 0, length);
            var str = Encoding.UTF8.GetString(strBytes);

            if (nameID == 1)
            {
                fontFamilyName = str;
            }
        }
    }

    private void ParsePostTable(BinaryReader reader, uint offset)
    {
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        var format = reader.ReadUInt32();
        var italicAngle = reader.ReadInt32();
        var underlinePosition = reader.ReadInt16();
        var underlineThickness = reader.ReadInt16();
        var isFixedPitch = reader.ReadUInt32();
        var minMemType42 = reader.ReadUInt32();
        var maxMemType42 = reader.ReadUInt32();
        var minMemType1 = reader.ReadUInt32();
        var maxMemType1 = reader.ReadUInt32();
    }

    public string GetFontFamilyName()
    {
        return fontFamilyName;
    }

    public ushort GetUnitsPerEm()
    {
        return unitsPerEm;
    }

    public short GetAscender()
    {
        return ascender;
    }

    public short GetDescender()
    {
        return descender;
    }

    public short GetLineGap()
    {
        return lineGap;
    }

    public ushort GetNumGlyphs()
    {
        return numGlyphs;
    }
}

// 上記のクラスを使用して、フォントファイルを解析することができます。
//var font = new TrueTypeFont("path/to/font.ttf");
/*
Console.WriteLine($"Font family name: {font.GetFontFamilyName()}");
Console.WriteLine($"Units per em: {font.GetUnitsPerEm()}");
Console.WriteLine($"Ascender: {font.GetAscender()}");
Console.WriteLine($"Descender: {font.GetDescender()}");
Console.WriteLine($"Line gap: {font.GetLineGap()}");
Console.WriteLine($"Number of glyphs: {font.GetNumGlyphs()}");
*/