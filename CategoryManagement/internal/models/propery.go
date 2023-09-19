package models

import (
	"database/sql/driver"
	"encoding/json"
	"errors"
)

type Property struct {
	Name     string `json:"name"`
	DataType string `json:"data_type"`
}

type Properties []Property

// Value implements the driver Valuer interface
func (p *Properties) Value() (driver.Value, error) {
	return json.Marshal(p)
}

// Scan implements the driver Scanner interface
func (p *Properties) Scan(value interface{}) error {
	bytes, ok := value.([]byte)
	if !ok {
		return errors.New("Scan: unsupported data type for Property")
	}

	return json.Unmarshal(bytes, p)
}
