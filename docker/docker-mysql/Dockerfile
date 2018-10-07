FROM openzipkin/jre-full:1.8.0_171

ENV ZIPKIN_VERSION 2.11.6

WORKDIR /mysql

ADD config/install.sh /mysql/install
RUN chmod +x /mysql/install
RUN /mysql/install

ADD config/configure.sh /mysql/configure
RUN chmod +x /mysql/configure
RUN /mysql/configure

ADD config/run.sh /mysql/run.sh
RUN chmod +x /mysql/run.sh
CMD /mysql/run.sh


EXPOSE 3306