namespace SmartFlow.Application.Reports;

public sealed record ReportFileDto(
    byte[] Content,
    string ContentType,
    string FileName);