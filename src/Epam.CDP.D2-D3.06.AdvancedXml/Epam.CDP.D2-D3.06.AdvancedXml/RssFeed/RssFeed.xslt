<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:x="http://library.by/catalog"
                exclude-result-prefixes="msxsl x">
    <xsl:output method="xml" indent="yes"/>

    <xsl:param name="Date" select="''"/>

    <xsl:template name="generateLink">
        <xsl:param name ="isbn">-</xsl:param>
        <xsl:element name="link" namespace="http://library.by/catalog">
            <xsl:variable name="url">
                <xsl:text>http://my.safaribooksonline.com/</xsl:text>
                <xsl:value-of select="$isbn"/>
                <xsl:text>/</xsl:text>
            </xsl:variable>
            <xsl:value-of select="$url"/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="x:catalog/x:book[x:isbn and x:genre='Computer']">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
            <xsl:call-template name ="generateLink">
                <xsl:with-param name="isbn" select="x:isbn"/>
            </xsl:call-template>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="x:catalog">
        <rss xmlns="http://library.by/catalog" version="2.0">
            <channel>
                <xsl:attribute name="transformDate">
                    <xsl:value-of select="$Date"/>
                </xsl:attribute>
                
                <title>RSS FEED</title>
                <description>New Books Announcement</description>
                <pubDate>
                    <xsl:value-of select="x:book/x:registration_date"/>
                </pubDate>
                <xsl:apply-templates/>
            </channel>
        </rss>
    </xsl:template>
</xsl:stylesheet>
