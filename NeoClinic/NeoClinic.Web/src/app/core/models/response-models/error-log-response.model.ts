export interface ErrorLogItem {
  id: string;
  message: string;
  stackTrace: string | null;
  source: string | null;
  path: string | null;
  method: string | null;
  statusCode: number;
  timestamp: string;
}

export interface GetErrorLogsResponse {
  items: ErrorLogItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
