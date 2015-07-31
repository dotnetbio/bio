Attribute VB_Name = "Module1"
Sub DisplayChart()
 Dim sequenceDataRange As Range

 For Each n In Application.ActiveSheet.Names
 Dim nameo As String
 nameo = n.Name
        SequenceData = "DATA_"
 Dim found As Integer
 found = InStr(1, nameo, SequenceData, vbTextCompare)
 If found > 0 Then Set sequenceDataRange = n.RefersToRange
 Next n
 
If sequenceDataRange Is Nothing Then
MsgBox ("Sequence data not found. Please run the macro on a sheet which has SequenceData pre-set selection")

Else

Dim currentWorkBook As Workbook, currentWorksheet As Worksheet
 
Set currentWorkBook = Application.ActiveWorkbook
Worksheets.Add(After:=Worksheets(Worksheets.Count)).Name = "ChartSheet" & Str(Worksheets.Count + 1)

Set currentWorksheet = Application.ActiveSheet
currentWorksheet.Cells(1, 1) = "A"
currentWorksheet.Cells(1, 2) = WorksheetFunction.CountIf(sequenceDataRange, "A")

currentWorksheet.Cells(2, 1) = "T"
currentWorksheet.Cells(2, 2) = WorksheetFunction.CountIf(sequenceDataRange, "T")

currentWorksheet.Cells(3, 1) = "G"
currentWorksheet.Cells(3, 2) = WorksheetFunction.CountIf(sequenceDataRange, "G")

currentWorksheet.Cells(4, 1) = "C"
currentWorksheet.Cells(4, 2) = WorksheetFunction.CountIf(sequenceDataRange, "C")


With ActiveSheet.ChartObjects.Add(Left:=100, Width:=375, Top:=75, Height:=225)
 .Chart.SetSourceData Source:=ActiveSheet.Range("A1:B4")
 .Chart.ChartType = xlColumnClustered
End With

End If

End Sub
