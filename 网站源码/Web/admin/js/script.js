function CheckAll(form)  
  {  
  for (var i=0;i<form.id.length;i++)  
    {  
    var e = form.id[i];  
    if (e.name != 'chkall') 
       e.checked = form.chkall.checked;
		form.submitid.disabled = (form.id.value = false);

    }  
  }


//自动缩小图片
function imgsize(obj,widthvalue,heightvalue)
{ 
var w; 
var h; 
w=obj.width; 
h=obj.height; 
if(obj.width>widthvalue){ 
do 
w=w-1; 
while (w>widthvalue) ; 
s=w/obj.width; 
obj.width=w; 
obj.height=h*s; 
h=obj.height 
} 
if(obj.height>heightvalue){ 
do 
h=h-1; 
while (h>heightvalue) ; 
s=h/obj.height; 
obj.width=w*s; 
obj.height=heightvalue; 
} 
} 


