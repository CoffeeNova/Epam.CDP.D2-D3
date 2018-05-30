<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:x="http://library.by/catalog"
                exclude-result-prefixes="msxsl x">
    <xsl:output method="html" indent="yes"/>

    <xsl:param name="Date" select="''"/>
    <xsl:key name="groups" match="/x:catalog/x:book" use="x:genre" />
    <xsl:variable name="totalBooks" select="position()" />

    <xsl:template match="x:catalog">
        <html>
            <header>
                <h1>
                    <xsl:text>Fund by genre for: </xsl:text>
                    <xsl:value-of select="$Date"/>
                </h1>
            </header>
            <body>
                <xsl:apply-templates select="x:book[generate-id() = generate-id(key('groups', x:genre)[1])]"/>
            </body>
            <footer>
                <h1>
                    <font size="4">
                        Total number of books of in the library:
                        <xsl:value-of select="count(x:book)"/>
                    </font>
                </h1>
            </footer>
        </html>
    </xsl:template>

    <xsl:template match="x:book">
        <xsl:variable name="genre" select="x:genre"/>
        <xsl:variable name="groupsVar" select="key('groups', x:genre)"/>
        <xsl:variable name="rowCount" select="count($groupsVar)"/>

        <table  border="1" >
            <caption align="left">
                <font size="5">
                    <xsl:value-of select="$genre"/>
                </font>
            </caption>
            <tr class="heading">
                <th>Author</th>
                <th>Title</th>
                <th>Publish date</th>
                <th>Registration date</th>
            </tr>
            <xsl:for-each select="$groupsVar">
                <tr>
                    <td>
                        <xsl:value-of select="x:author"/>
                    </td>
                    <td>
                        <xsl:value-of select="x:title"/>
                    </td>
                    <td>
                        <xsl:value-of select="x:publish_date"/>
                    </td>
                    <td>
                        <xsl:value-of select="x:registration_date"/>
                    </td>
                </tr>
            </xsl:for-each>
            <tr>
                <td colspan="4">
                    Total number of books of the <xsl:value-of select="$genre"/> genre:
                    <xsl:value-of select="$rowCount" />
                </td>
            </tr>
        </table>
    </xsl:template>
</xsl:stylesheet>
