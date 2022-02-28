/* 載入縣市與代碼到地址<select>中，順序來自郵局網站 */

//let citys = [['基隆市', 'KLU'], ['臺北市', 'TPE'], ['新北市', 'TPH'], ['桃園市', 'TYC'], ['新竹市', 'HSC'], ['新竹縣', 'HSH'], ['苗栗縣', 'MAL'], ['臺中市', 'TXG'], ['彰化縣', 'CWH'], ['南投縣', 'NTO'], ['雲林縣', 'YLH'], ['嘉義市', '	CYI'], ['嘉義縣', 'CHY'], ['臺南市', 'TNN'], ['高雄市', 'KHH'], ['屏東縣', 'IUH'], ['臺東縣', 'TTT'], ['花蓮縣', 'HWA'], ['宜蘭縣', 'ILN'], ['澎湖縣', 'PEH'], ['金門縣', 'KMN'], ['連江縣', '	LNN']];
let citys = ['基隆市', '臺北市', '新北市', '桃園市', '新竹市', '新竹縣', '苗栗縣', '臺中市', '彰化縣', '南投縣', '雲林縣', '嘉義市', '嘉義縣', '臺南市', '高雄市', '屏東縣', '臺東縣', '花蓮縣', '宜蘭縣', '澎湖縣', '金門縣', '連江縣'];
let domCity = document.getElementById('seleAddr');
for (let city of citys) {
    let nodeOption = document.createElement('option');
    //nodeOption.textContent = city[0];
    //nodeOption.setAttribute('value', city[1]);
    nodeOption.textContent = city;
    nodeOption.setAttribute('value', city);

    domCity.appendChild(nodeOption);
}
