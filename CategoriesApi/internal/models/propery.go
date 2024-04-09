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
	if p == nil {
		return nil, nil
	}
	return json.Marshal(p)
}

// Scan implements the driver Scanner interface
func (p *Properties) Scan(value interface{}) error {
	if value == nil {
		return nil
	}

	bytes, ok := value.([]byte)
	if !ok {
		return errors.New("Scan: unsupported data type for Property")
	}

	return json.Unmarshal(bytes, p)
}
