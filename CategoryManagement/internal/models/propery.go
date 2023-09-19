package models

type Property struct {
	Name     string      `json:"name"`
	DataType string      `json:"data_type"`
	Value    interface{} `json:"value"`
}
