          function Class1Change(str,f1,f2){
              var classid=$('#'+f1).val();
              if(classid==null||classid==""||classid=="0")
              {
              CountryDel(f2);//清空DropDownList
              $('#'+f2).attr("disabled",true);
              //CountryDel("Class3");//清空DropDownList
              //$('#'+f3).attr("disabled",true);
              return false;
              }
              
              if(classid=="")
              {
                $('#'+f2).val("");
                $('#'+f2).attr("disabled",true);
                //$('#'+f3).val("");
                //$('#'+f3).attr("disabled",true);
              return false;
              }
              
            $('#'+f2).attr("disabled",false);
          
            $.get(str + "Inc/Select.aspx?ParentId="+classid, function(data){
                  //分割并写入DropDownList
                  CountryDel(f2);//清空DropDownList
                  //CountryDel("Class3");//清空DropDownList
                  var stringarray=data.split("|");
                  if(stringarray!="")
                  {
                      for(var i=0;i<stringarray.length;i++){//重写DropDownList
                      //拆分内部数组
                      var optionarray=stringarray[i].split(",");
                      var newoption=document.createElement("option");
                      newoption.text=optionarray[0];
                      newoption.value=optionarray[1];
                      document.getElementById(f2).options.add(newoption);
                      }
                  }
             });
          }
          

          
          function Class2Change(str,f2,f3){
              var classid=$('#'+f2).val();
              
                  if(classid==null||classid==""||classid=="0")
                  {
                      CountryDel(f3);//清空DropDownList
                      ('#'+f3).attr("disabled",true);
                      return false;
                  }
                  
                  if(classid=="")
                  {
                    $('#'+f3).val("");
                    $('#'+f3).attr("disabled",true);
                    return false;
                  }
                  
              $('#'+f3).attr("disabled",false);
              $.get(str + "Inc/Select.aspx?ParentId="+classid, function(data){
                      countrystring = data;
                      //分割并写入DropDownList
                      CountryDel(f3);//清空DropDownList
                      //CountryDel("Class3");//清空DropDownList
                      var stringarray=data.split("|");
                      if(stringarray!="")
                      {
                          for(var i=0;i<stringarray.length;i++){//重写DropDownList
                          //拆分内部数组
                          var optionarray=stringarray[i].split(",");
                          var newoption=document.createElement("option");
                          newoption.text=optionarray[0];
                          newoption.value=optionarray[1];
                          document.getElementById(f3).options.add(newoption);
                          }
                      }
               }); 
          }
          
          
          function CountryDel(AreaName){//清空DropDownList
              $('#'+AreaName).empty();
              $('#'+AreaName).append("<option value=''>-请选择-</option>");
          }