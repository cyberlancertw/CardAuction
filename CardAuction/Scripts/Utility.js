
// Define a plugin to provide data labels
Chart.plugins.register({
    afterDatasetsDraw: function (chart, easing) {
        // To only draw at the end of animation, check for easing === 1
        var ctx = chart.ctx;

        chart.data.datasets.forEach(function (dataset, i) {
            var meta = chart.getDatasetMeta(i);
            if (!meta.hidden) {
                meta.data.forEach(function (element, index) {
                    // Draw the text in black, with the specified font
                    ctx.fillStyle = 'rgb(0, 0, 0)';

                    var fontSize = 16;
                    var fontStyle = 'normal';
                    var fontFamily = 'Helvetica Neue';
                    ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

                    // Just naively convert to string for now
                    var dataString = "";
                    var titleText = chart.options.title.text;
                    if (titleText.indexOf("%") > 0) {
                        dataString = dataset.data[index].toString() + " %";
                    }
                    else {
                        dataString = dataset.data[index].toString();
                    }

                    // Make sure alignment settings are correct
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';

                    var padding = 7;
                    var position = element.tooltipPosition();
                    if (chart.config.type == "horizontalBar") {
                        ctx.fillText(dataString, position.x + 25, position.y);
                    }
                    else {
                        ctx.fillText(dataString, position.x, position.y - (fontSize / 2) - padding);
                    }
                });
            }
        });
    }
});
