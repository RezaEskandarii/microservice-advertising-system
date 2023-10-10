import axios from "axios";


class HttpService {
    constructor() {
        this.accessToken = localStorage.getItem('accessToken');
        this.instance = axios.create();

        this.instance.interceptors.request.use((config) => {
            if (this.accessToken) {
              ///  config.headers['Authorization'] = `Bearer ${this.accessToken}`;
            }
            return config;
        });
    }

    get(url, config): Promise<> {
        return this.instance.get(url, config);
    }

    post(url, data, config): Promise<> {
        return this.instance.post(url, data, config);
    }

    put(url, data, config): Promise<> {
        return this.instance.put(url, data, config);
    }

    delete(url, config): Promise<> {
        return this.instance.delete(url, config);
    }
}

export default HttpService;