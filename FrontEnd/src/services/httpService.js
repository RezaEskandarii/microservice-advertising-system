import axios from "axios";


class HttpService {
    constructor() {
        this.accessToken = localStorage.getItem('accessToken');
        this.instance = axios.create();

        // Add request interceptor
        this.instance.interceptors.request.use((config) => {
            if (this.accessToken) {
                config.headers['Authorization'] = `Bearer ${this.accessToken}`;
            }
            return config;
        });
    }

    get(url, config) {
        return this.instance.get(url, config);
    }

    post(url, data, config) {
        return this.instance.post(url, data, config);
    }

    put(url, data, config) {
        return this.instance.put(url, data, config);
    }

    delete(url, config) {
        return this.instance.delete(url, config);
    }
}