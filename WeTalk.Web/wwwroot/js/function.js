
//***************检测是否为数字或整数*************
//函数名isNumber()
//作用	检测是否数值
//参数	str：被检验的字串
//返回值	true/false
//************************************************
function isNumber(str) {
    return str.search(/^\d*\.?\d+jQuery|^\d+\.?\d*$/) == 0
}

//***************检测是否为数字或整数*************
//函数名isInt()
//作用	检测是否整数
//参数	str：被检验的字串
//返回值	true/false
//************************************************
function isInt(str) {
    return str.search(/^\d+$/) == 0
}


//***************检测是否手机号*************
//函数名isMobile()
//作用	检测是否手机号
//参数	str：被检验的字串
//返回值	true/false
//************************************************
function isMobile(str) {
	return /^1[3-9]+\d{9}$/.test(str);
}

//***************检测是否座机号*************
//函数名isTel()
//作用	检测是否座机号
//参数	str：被检验的字串
//返回值	true/false
//************************************************
function isTel(str) {
	return /(^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$)|(^(()|(\d{3}\-))?(1[358]\d{9})$)/.test(str);
}

//***************检测是否身份证号*************
//函数名isCard()
//作用	检测是否身份证号
//参数	str：被检验的字串
//返回值	true/false
//************************************************
function isCard(str) {
	switch (value.length) {
		case 15:
			return /^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/.test(str);
			break;
		case 18:
			return /^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$/.test(str);
			break;
		default:
			return false;
			break;
	}
}

//***************检测是否Email*************
//函数名isEmail()
//作用	检测是否Email
//参数	str：被检验的字串
//返回值	true/false
//************************************************
function isEmail(str) {
	// /^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;
	return /^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$/.test(str);
}