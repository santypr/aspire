const HTMLWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = (env) => {
  const port = parseInt(env.PORT) || 3000;
  return {
    entry: "./src/index.tsx",
    devServer: {
      port: port,
      allowedHosts: "all",
      proxy: [
        {
          context: ["/api"],
          target:
            process.env.services__apiservice__https__0 ||
            process.env.services__apiservice__http__0 ||
            process.env.REACT_APP_API_URL ||
            "http://localhost:5304",
          pathRewrite: { "^/api": "" },
          secure: false,
          changeOrigin: true,
        },
      ],
    },
    output: {
      path: `${__dirname}/dist`,
      filename: "bundle.js",
    },
    resolve: {
      extensions: [".js", ".jsx", ".ts", ".tsx"],
    },
    plugins: [
      new HTMLWebpackPlugin({
        template: "./public/index.html",
        favicon: "./public/favicon.ico",
        templateParameters: {
          PUBLIC_URL: "",
        },
      }),
      new CopyWebpackPlugin({
        patterns: [
          {
            from: "public",
            to: ".",
            globOptions: {
              ignore: ["**/index.html"], // HTMLWebpackPlugin handles this
            },
          },
        ],
      }),
    ],
    module: {
      rules: [
        {
          test: /\.(js|jsx|ts|tsx)$/,
          exclude: /node_modules/,
          use: {
            loader: "babel-loader",
            options: {
              presets: [
                "@babel/preset-env",
                ["@babel/preset-react", { runtime: "automatic" }],
                "@babel/preset-typescript",
              ],
            },
          },
        },
        {
          test: /\.css$/,
          exclude: /node_modules/,
          use: ["style-loader", "css-loader"],
        },
      ],
    },
  };
};