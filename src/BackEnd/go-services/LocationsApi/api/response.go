package api

type ApiResponse struct {
	Status     int         `json:"status"`
	Message    string      `json:"message"`
	Data       interface{} `json:"data,omitempty"`
	Pagination *Pagination `json:"pagination,omitempty"`
	Error      *Error      `json:"error"`
}

type Error struct {
	Type     string `json:"type"`
	Title    string `json:"title"`
	Status   int    `json:"status"`
	Detail   string `json:"detail,omitempty"`
	Instance string `json:"instance,omitempty"`
}

type Pagination struct {
	Page       int `json:"page"`
	PerPage    int `json:"per_page"`
	PagesTotal int `json:"pages_total"`
}
