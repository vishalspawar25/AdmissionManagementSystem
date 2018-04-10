var source = $("#Batchtemplate").html();
var template = Handlebars.compile(source);
var array = new Array();
var ColorArray = ['#3edfdf', '#cddd19', '#4281f1', '#f80570', '#1abb9c', '#7310dd'];
$(function () {
    
    Batches();
    $('#Courses').change(function () {
        Batches();
    });
});
function LoadBoxes() {
    

};
function Batches() {
    var courseId = $('#Courses option:selected').val();
    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: SiteUrl+'/Dashboard/LoadBatches',
        data: { CourseId: courseId},
        success: function (data) {

            
            array = { "Batches": data.Batches };          
            $('#BatchBox').html(template(array));

            $.each(data.Batches, function (key, val)
            {
                console.log("'" + ColorArray[key] + "'");
                
                $("#Batch" + key).css('background-color', ColorArray[key]);
            });
                
           
           
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('error in loading batches.');
        }
    });

  
};

function ShowStudents(BatchId,page)
{
    url = SiteUrl + "/" + page + "?id=" + BatchId;
    window.open(url,'_Blank','location=No');

}