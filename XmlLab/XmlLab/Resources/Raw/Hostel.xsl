<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" encoding="UTF-8" indent="yes"/>

    <xsl:template match="/Hostel">
        <html>
            <head>
                <meta charset="UTF-8"/>
                <title>Гуртожиток</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 24px; }
                    .card { border: 1px solid #ccc; border-radius: 8px; padding: 12px; margin-bottom: 16px; }
                    table { border-collapse: collapse; width: 100%; }
                    th, td { border: 1px solid #999; padding: 4px 8px; text-align: left; }
                    th { background: #eee; }
                </style>
            </head>
            <body>
                <h1>Результати пошуку</h1>
                <p>Знайдено записів: <xsl:value-of select="count(Resident)"/></p>
                <xsl:for-each select="Resident">
                    <div class="card">
                        <h3><xsl:value-of select="@Name"/></h3>
                        <ul>
                            <xsl:for-each select="@*[name() != 'Name']">
                                <li><b>
                                    <xsl:choose>
                                        <xsl:when test="name()='Faculty'">Факультет</xsl:when>
                                        <xsl:when test="name()='Department'">Кафедра</xsl:when>
                                        <xsl:when test="name()='Course'">Курс</xsl:when>
                                        <xsl:when test="name()='Phone'">Телефон</xsl:when>
                                        <xsl:otherwise><xsl:value-of select="name()"/></xsl:otherwise>
                                    </xsl:choose>: </b><xsl:value-of select="."/></li>                            </xsl:for-each>
                        </ul>
                        <table>
                            <tr><th>Корпус</th><th>Кімната</th><th>Заселення</th><th>Виселення</th></tr>
                            <xsl:for-each select="Residence">
                                <tr>
                                    <td><xsl:value-of select="@Building"/></td>
                                    <td><xsl:value-of select="@Room"/></td>
                                    <td><xsl:value-of select="@CheckIn"/></td>
                                    <td><xsl:value-of select="@CheckOut"/></td>
                                </tr>
                            </xsl:for-each>
                        </table>
                    </div>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>