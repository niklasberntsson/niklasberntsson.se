/// <reference path="jquery-1.10.2.min.js" />
$(document).ready(function () {
  
    $("#btnCreateNewWeekPlan").click(function () {
        $("#newWeekPlan").toggle();
    });

    $("#btnSubmitNewWeekPlan").click(function () {
        var name = $("#name").val();
        var active = 0;
        if ($("#active").is(":checked")) {
            active = 1;
        }
        $.get("WeekPlan/AddWeekPlan", { name: name, active:active }, function (data) {
            window.location = "/WeekPlan/Index";
        });
        
    });

    $("#mealIndex").change(function () {
        $("#recipiesList").empty();
        
        var id = $("#mealIndex").val();

        $.get("Home/GetRecipiesWithMealId", { mealId: id }, function (data) {
            $.each(data, function(key, val){
                //$("#recipiesList").append("<li class='list-group-item recipe' id='recipe_" + val.Id + "'>" + val.Name + "</li>");
                writeRecipeLine(val);
            });
        });
    });

    $('#search').keyup(function () {
        var val = $("#search").val();

        

        //only search on three letters or more
        if (val.length > 2) {
            $("#recipiesList").empty();
            $.get("Home/Search", { query: val }, function (data) {
                $.each(data, function (key, val) {
                    writeRecipeLine(val);
                });
            });
        }
        else if (val.length == 0) {
            $("#recipiesList").empty();
            $.get("Home/GetRecipiesWithMealId", { mealId: 0 }, function (data) {
                $.each(data, function (key, val) {
                    writeRecipeLine(val);
                });
            });
        }
    });

    $(".weekPlanSelect").on("change", function(event){
        //Get value from select
        //event.target.id

        var selectedRecipe = $("#" + event.target.id + " option:selected");
        //alert("Namn: " + selectedRecipe.text() + ", Id: " + selectedRecipe.attr("id") + ", From select: " + event.target.id);
        //Get weekplan id
        var weekPlanId = $("#weekPlanId").val();

        $.get("/WeekPlan/SaveItem", { weekPlanId: weekPlanId, recipeId: selectedRecipe.attr("id"), day: event.target.id }, function (response) {
            alert(response);
        });
        
    });

    $(".addToWeekPlan").on("click", function (event) {
        //Check if there is an active weekPlan
        $.get("/WeekPlan/ActiveWeekPlanExists", function (data) {
            if (data > 0) {
                //get recipe id
                var elementId = event.target.id;
                var id = elementId.split("_")[1];
                
                if (id > 0) {
                    $.get("/WeekPlan/AddRecipe", { weekPlanId: data, recipeId: id }, function (response) {
                        if (response == 200) {
                            $("#modalWeekPlanHeader").text("Recept tillagt");
                            $("#myModal").modal();
                            setTimeout(function () {
                                $("#myModal").modal('hide');
                            }, 2000);
                        }
                    });
                }
            }
        });
    });
    
});

function writeRecipeLine(val) {
    
    $("#recipiesList").append("<li class='list-group-item recipe dropdown-toggle' data-toggle='dropdown' id='recipe_" + val.Id + "'>" +
        "<a href='#' data-toggle='dropdown' class='dropdown-toggle'>" + val.Name + "</a>" +
        "<ul class='dropdown-menu'>" +
                "<li><a href='" + val.Link + "' onclick='followLink(this)' target='_blank'>Följ länk</a></li>" +
                "<li class='divider'></li>" +
                "<li><a href='#'>Lägg till i veckomatsedeln</a></li>" +
                "<li class='divider'></li>" +
                "<li><a href='#'>Ändra recept</a></li>" +
                "<li><a href='#'>Markera som död länk</a></li>" +
                "<li><a href='#'>Dela</a></li></ul>" +
        "</li>");
}

function followLink(element) {
    window.open(element, "_blank");
}

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    ev.target.appendChild(document.getElementById(data));
    
    console.log(data); //week item Id
    console.log(ev.target.id); //week day
    var weekPlanId = $("#weekPlanId").val();

    $.get("/WeekPlan/AddItemToWeekPlan", { weekPlanId: weekPlanId, weekItemId: data, day: ev.target.id }, function (data) {
        
    });

}
