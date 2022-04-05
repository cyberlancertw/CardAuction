window.chartColors = {
    red: 'rgb(255, 99, 132)',
    pink : 'rgb(255,75,50)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)',
    orange: 'rgb(255, 159, 64)',
};

var colors = [];
colors[0] = 'red';
colors[1] = 'LightPink';
colors[2] = 'yellow';
colors[3] = 'green';
colors[4] = 'blue';
colors[5] = 'purple';
colors[6] = 'Aqua';
colors[7] = 'DarkTurquoise';
colors[8] = 'Gold';
colors[9] = 'LightCoral';
colors[10] = 'MediumAquaMarine';
colors[11] = 'MintCream';
colors[12] = 'orange';


function getRandomColor() {
    var index = Math.floor(Math.random() * 13);

    return colors[index];
}