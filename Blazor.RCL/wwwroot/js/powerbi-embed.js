window.powerbiEmbed = {
    embedReport: function (embedToken, embedUrl, reportId, filterTable, filterColumn, filterValue) {

        var reportContainer = document.getElementById('reportContainer');

        if (!reportContainer) {
            return;
        }

        var models = window['powerbi-client'].models;

        const groupFilter = {
            $schema: "http://powerbi.com/product/schema#basic",
            target: {
                table: filterTable,
                column: filterColumn
            },
            operator: "In",
            values: [filterValue]
        };

        var config = {
            type: 'report',
            tokenType: models.TokenType.Embed,
            accessToken: embedToken,
            embedUrl: embedUrl,
            id: reportId,
            filters: [groupFilter],
            permissions: models.Permissions.View,
            settings: {
                filterPaneEnabled: false,
                navContentPaneEnabled: true,
                layoutType: models.LayoutType.Custom,
                customLayout: {
                    displayOption: models.DisplayOption.FitToWidth
                }
            }
        };
        window.powerbi.embed(reportContainer, config);
    }
};