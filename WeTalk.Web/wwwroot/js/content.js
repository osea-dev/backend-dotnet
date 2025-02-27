var $parentNode = window.parent.document;

function $childNode(name) {
    return window.frames[name]
}

// tooltips
$('.tooltip-demo').tooltip({
    selector: "[data-toggle=tooltip]",
    container: "body"
});

// 使用animation.css修改Bootstrap Modal
$('.modal').appendTo("body");

$("[data-toggle=popover]").popover();

//折叠ibox
$('.collapse-link').click(function () {
    var ibox = $(this).closest('div.ibox');
    var button = $(this).find('i');
    var content = ibox.find('div.ibox-content');
    content.slideToggle(200);
    button.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
    ibox.toggleClass('').toggleClass('border-bottom');
    setTimeout(function () {
        ibox.resize();
        ibox.find('[id^=map-]').resize();
    }, 50);
});

//关闭ibox
$('.close-link').click(function () {
    var content = $(this).closest('div.ibox');
    content.remove();
});

//判断当前页面是否在iframe中
if (top == this) {
    var gohome = '<div class="gohome"><a class="animated bounceInUp" href="index.html?v=4.0" title="返回首页"><i class="fa fa-home"></i></a></div>';
    $('body').append(gohome);
}

//animation.css
function animationHover(element, animation) {
    element = $(element);
    element.hover(
        function () {
            element.addClass('animated ' + animation);
        },
        function () {
            //动画完成之前移除class
            window.setTimeout(function () {
                element.removeClass('animated ' + animation);
            }, 2000);
        });
}

//拖动面板
function WinMove() {
    var element = "[class*=col]";
    var handle = ".ibox-title";
    var connect = "[class*=col]";
    $(element).sortable({
            handle: handle,
            connectWith: connect,
            tolerance: 'pointer',
            forcePlaceholderSize: true,
            opacity: 0.8,
        })
        .disableSelection();
};

//eric
// 原JScript 文件

function CheckAll(form) {
    for (var i = 0; i < form.elements.length; i++) {
        var e = form.elements[i];
        if (e.name != 'chkall') e.checked = true;
    }
}
function NoCheckAll(form) {
    for (var i = 0; i < form.elements.length; i++) {
        var e = form.elements[i];
        if (e.name != 'chkall') e.checked = false;
    }
}
function AnCheckAll(form) {
    for (var i = 0; i < form.elements.length; i++) {
        var e = form.elements[i];
        if (e.name != 'chkall') {
            e.checked = !e.checked;
        }
    }
}
function isdel() {
    var ok = confirm("确定删除吗?删除后数据不能恢复!请小心删除!")
    if (!ok) return false
    else return true;
}
function isExc() {
    var ok = confirm("确定执行操作吗?!")
    if (!ok) return false
    else return true;

}

function SelectAll(form) {
    for (var i = 0; i < form.elements.length; i++) {
        var e = form.elements[i];
        if (e.name == 'id')
            e.checked = form.ChkAll.checked;
        if (e.name == 'Tree_id')
            e.checked = form.ChkAll.checked;
    }
}

function chkselect() {
    if (confirm("确定执行操作吗？")) {
        var chked;
        chked = false;
        for (var i = 0; i < form1.elements.length; i++) {
            var e = form1.elements[i];
            if (e.name == 'id' && e.checked == true) {
                chked = true;
                break;
            }
        }
        if (chked == false) {
            alert("请选择要操作的信息！");
            return false;
        }
        return true;
    }
    else {
        return false;
    }
}
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
